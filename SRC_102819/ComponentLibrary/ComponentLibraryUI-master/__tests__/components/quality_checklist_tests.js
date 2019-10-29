import { QualityChecklist } from '../../src/components/checklists/quality_checklist';
import chai, {expect} from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';

chai.use(chaiEnzyme());

describe('QualityChecklist', () => {
    let props;
    let wrapper;

    beforeEach(() => {
        props = {
            details: {
                checkListId: 'SMPQ0001',
                id: '23',
                template: 'QualityChecklist',
                title: 'Quality Checklist - Masonry - Solid Concrete Block',
                content: {
                    entries: [{
                        cells: [{
                            key: '0 0',
                            value: 'S.No'
                        }]
                    },
                        {
                            cells: [{
                                key: '1 0',
                                value: '1'
                            }]
                        }]
                }
            },
        };
        wrapper = shallow(<QualityChecklist {...props}/>);
    });

    it('should render error message if entries in details are not found', () => {
        props.details.content.entries = null;
        wrapper = shallow(<QualityChecklist {...props}/>);
        const result = wrapper.find('div');
        expect(result.contains('QualityChecklist checklist data is not found')).to.equal(true);
    });

    it('should render header data', () => {
        expect(wrapper).to.contain(<b>Project Name : </b>);
        expect(wrapper).to.contain(<b>Quality Checklist - Masonry - Solid Concrete Block</b>);
        expect(wrapper).to.contain(<b>SMPQ0001</b>);
        expect(wrapper).to.contain(<b>TE/</b>);
    });

    it('should render Content, Remarks, Footer', () => {
        expect(wrapper).to.have.descendants('Content');
        const result = wrapper.find('Content');
        expect(result).to.have.prop('entries').deep.equal(props.details.content.entries);
        expect(wrapper).to.have.descendants('Remarks');
        expect(wrapper).to.have.descendants('Footer');
    });
});

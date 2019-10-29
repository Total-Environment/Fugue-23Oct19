import {SpecificationsSheet } from '../../src/components/checklists/specifications_sheet';
import chai, {expect} from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';

chai.use(chaiEnzyme());

describe('SpecificationsSheet', () => {
    let props;
    let wrapper;

    beforeEach(() => {
        props = {
            details: {
                checkListId: 'SRID0001',
                id: '23',
                template: 'SpecificationsSheet',
                title: 'Material Test Sheet - Synthetic Flooring (Class 23)',
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
        wrapper = shallow(<SpecificationsSheet {...props}/>);
    });

    it('should render error message if entries in details are not found', () => {
        props.details.content.entries = null;
        wrapper = shallow(<SpecificationsSheet {...props}/>);
        const result = wrapper.find('div');
        expect(result.contains('SpecificationsSheet checklist data is not found')).to.equal(true);
    });

    it('should render header data', () => {

        expect(wrapper).to.contain(<b>Project/Store Name:</b>);
        expect(wrapper).to.contain(<b>Material Test Sheet - Synthetic Flooring (Class 23)</b>);
        expect(wrapper).to.contain(<b>TE/</b>);
        expect(wrapper).to.contain(<b>SRID0001</b>);
    });

    it('should render Content', () => {
        expect(wrapper).to.have.descendants('Content');
        const result = wrapper.find('Content');
        expect(result).to.have.prop('entries').deep.equal(props.details.content.entries);
    });
});

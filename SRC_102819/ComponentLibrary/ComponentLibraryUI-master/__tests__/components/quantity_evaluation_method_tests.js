import {QuantityEvaluationMethod } from '../../src/components/checklists/quantity_evaluation_method';
import chai, {expect} from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';

chai.use(chaiEnzyme());

describe('QuantityEvaluationMethod', () => {
    let props;
    let wrapper;

    beforeEach(() => {
        props = {
            details: {
                checkListId: 'SRID0001',
                id: '23',
                template: 'QuantityEvaluationMethod',
                title: 'Inspection chekclist-Aluminium Products',
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
        wrapper = shallow(<QuantityEvaluationMethod {...props}/>);
    });

    it('should render error message if entries in details are not found', () => {
        props.details.content.entries = null;
        wrapper = shallow(<QuantityEvaluationMethod {...props}/>);
        const result = wrapper.find('div');
        expect(result.contains('QuantityEvaluationMethod checklist data is not found')).to.equal(true);
    });

    it('should render header data', () => {

        expect(wrapper).to.contain(<b>Project/Store Name:</b>);
        expect(wrapper).to.contain(<b>Inspection chekclist-Aluminium Products</b>);
        expect(wrapper).to.contain(<b>SRID0001</b>);
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
